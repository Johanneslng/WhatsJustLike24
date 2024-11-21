import {
  Component,
  OnInit,
  OnDestroy,
  HostListener,
  ElementRef,
  ViewChild
} from '@angular/core';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { GetMoviesService } from 'src/app/services/apiCalls/get-movies.service';
import { GetGamesService } from 'src/app/services/apiCalls/get-games.service';
import { SimilarMovie } from 'src/app/models/SimilarMovie';
import { SimilarContent } from 'src/app/models/SimilarContent';
//import { MovieWithDetails } from 'src/app/models/MovieWithDetails';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { ContentType } from 'src/app/models/Enums/ContentType';
import { Subscription, BehaviorSubject, combineLatest } from 'rxjs';
import { distinctUntilChanged, switchMap } from 'rxjs/operators';
import { DisplaySimilarityDetailsComponent } from '../display-similarity-details/display-similarity-details.component';
import { MatDialog } from '@angular/material/dialog';
import { IContentService } from 'src/app/models/IContentService';
import { ContentWithDetails, GameWithDetails, MovieWithDetails } from 'src/app/models/ContentWithDetails';


@Component({
  selector: 'app-display-content',
  templateUrl: './display-content.component.html',
  styleUrl: './display-content.component.scss'
})
export class DisplayContentComponent implements OnInit, OnDestroy {
  private searchState$ = new BehaviorSubject<{ value: string; type: ContentType }>({
    value: '',
    type: ContentType.Movies,
  });

  selectedType: ContentType = ContentType.Movies;
  displayedColumns: string[] = ['image', 'title', 'averageSimilarityScore', 'ratingCount'];
  imageUrl: string = 'https://wjlimg.blob.core.windows.net/img/';
  similarContent: SimilarContent[] = []; 
  searchContent: ContentWithDetails | null = null;
  hoveredContent: SimilarContent | null = null;
  similarTitle: string = '';
  currentSearchValue: string = '';

  isLoading: boolean = false;
  hasSearched: boolean = false;
  selectedIndex: number = 0;

  private serviceMapping: { [key in ContentType]: IContentService };
  private subscriptions: Subscription[] = [];
  private fetchActions: {
    [key in ContentType]: {
      fetchDetails: (searchValue: string) => void;
      fetchContent: (searchValue: string) => void;
      fetchSimilarTitle: (searchValue: string) => void;
    };
  };

  constructor(
    private getMoviesService: GetMoviesService,
    private getGamesService: GetGamesService,
    private setSearchValueService: SetSearchValueService,
    public dialog: MatDialog
  )
  {
    this.fetchActions = {
      [ContentType.Movies]: {
        fetchDetails: (searchValue: string) => this.fetchDetails(this.serviceMapping[ContentType.Movies], searchValue),
        fetchContent: (searchValue: string) => this.fetchContent(this.serviceMapping[ContentType.Movies], searchValue),
        fetchSimilarTitle: (searchValue: string) => this.fetchSimilarTitle(this.serviceMapping[ContentType.Movies], searchValue),
      },
      [ContentType.Games]: {
        fetchDetails: (searchValue: string) => this.fetchDetails(this.serviceMapping[ContentType.Games], searchValue),
        fetchContent: (searchValue: string) => this.fetchContent(this.serviceMapping[ContentType.Games], searchValue),
        fetchSimilarTitle: (searchValue: string) => this.fetchSimilarTitle(this.serviceMapping[ContentType.Games], searchValue),
      },
      [ContentType.Shows]: { //TODO Exchange to Shows
        fetchDetails: (searchValue: string) => this.fetchDetails(this.serviceMapping[ContentType.Games], searchValue),
        fetchContent: (searchValue: string) => this.fetchContent(this.serviceMapping[ContentType.Games], searchValue),
        fetchSimilarTitle: (searchValue: string) => this.fetchSimilarTitle(this.serviceMapping[ContentType.Games], searchValue),
      },
      [ContentType.Books]: { //TODO Exchange to books
        fetchDetails: (searchValue: string) => this.fetchDetails(this.serviceMapping[ContentType.Games], searchValue),
        fetchContent: (searchValue: string) => this.fetchContent(this.serviceMapping[ContentType.Games], searchValue),
        fetchSimilarTitle: (searchValue: string) => this.fetchSimilarTitle(this.serviceMapping[ContentType.Games], searchValue),
      },
    };

    this.serviceMapping = {
      [ContentType.Movies]: this.getMoviesService,
      [ContentType.Games]: this.getGamesService,
      [ContentType.Shows]: this.getGamesService,
      [ContentType.Books]: this.getGamesService
    };
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.setSearchValueService
        .getSearchState()
        .pipe(distinctUntilChanged())
        .subscribe((state) => this.searchState$.next(state))
    );

    this.subscriptions.push(
      this.searchState$
        .pipe(
          distinctUntilChanged(),
          switchMap(({ value, type }) => {
            if (!value) return [];
            this.isLoading = true;
            const service = this.serviceMapping[type];
            return combineLatest([
              service.fetchDetails(value),
              service.fetchContent(value),
              service.fetchSimilarTitle(value),
            ]);
          })
        )
        .subscribe(
          ([details, content, similarTitle]) => {
            this.searchContent = this.processDetails(details);
            this.similarContent = this.processSimilarContent(content, this.searchState$.value.type);
            this.similarTitle = similarTitle;
            this.isLoading = false;
          },
          (error) => {
            console.error('Error fetching data:', error);
            this.isLoading = false;
          }
        )
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  private processDetails(details: ContentWithDetails): ContentWithDetails {
    const { type } = this.searchState$.value;
    const imagePath = this.getImagePath(type);
    return { ...details, image: `${this.imageUrl}${imagePath}${details.image}` };
  }

  private processSimilarContent(content: SimilarContent[], type: ContentType): SimilarContent[] {
    const imagePath = this.getImagePath(type);
    return content.map((item) => ({
      ...item,
      image: `${this.imageUrl}${imagePath}${item.image}`,
    }));
  }

  private getImagePath(type: ContentType): string {
    switch (type) {
      case ContentType.Movies:
        return 'movies/';
      case ContentType.Games:
        return 'games/';
      default:
        return '';
    }
  }

  private fetchDetails(service: IContentService, searchValue: string): void {
    this.isLoading = true;

    service.fetchDetails(searchValue).subscribe(
      (response: ContentWithDetails) => {
        console.log('Fetched details:', response);
        this.isLoading = false;

        switch (response.type) {
          case 'Movie':
            this.searchContent = {
              ...response,
              image: this.imageUrl + 'movies/' + response.image
            };
            console.log('Processed movie details:', this.searchContent);
            break;

          case 'Game':
            this.searchContent = {
              ...response,
              image: this.imageUrl + 'games/' + response.image
            };
            console.log('Processed game details:', this.searchContent);
            break;

          default:
            console.error('Unknown content type:', response);
            break;
        }
      },
      (error) => {
        console.error('Error fetching details:', error);
        this.isLoading = false;
      }
    );
  }


  private fetchContent(service: IContentService, searchValue: string): void {
    this.isLoading = true;

    service.fetchContent(searchValue).subscribe(
      (response: SimilarContent[]) => {
        console.log('Fetched content:', response);

        switch (this.selectedType) {
          case ContentType.Movies:
            this.similarContent = response.map(content => ({
              ...content,
              image: this.imageUrl + 'movies/' + content.image 
            }));
            break;

          case ContentType.Games:
            this.similarContent = response.map(content => ({
              ...content,
              image: this.imageUrl + 'games/' + content.image
              
            }));
            break;

          default:
            console.error('Unknown content type:', response);
            break;
        }
        this.similarContent = response.map(content => ({
          ...content,
          image: this.imageUrl + 'movies/' + content.image //TODO variablize
        }));
        this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching content:', error);
        this.isLoading = false;
      }
    );
  }

  private fetchSearchContentDetails(service: IContentService, searchValue: string): void {
    this.searchContent = {
      type: 'Movie',
      name: '',
      genre: '',
      director: '',
      image: '',
      description: '',
    };

    service.fetchDetails(searchValue)
      .subscribe((response: ContentWithDetails) => {
        this.searchContent = {
          ...response,
          image: this.imageUrl + 'games/' + response.image
        };
      }, error => {
        console.error(error);
      });
  }

  private fetchSimilarTitle(service: IContentService, searchValue: string): void {
    service.fetchSimilarTitle(searchValue).subscribe(
      (response: string) => {
        console.log('Fetched similar title:', response);
        this.similarTitle = response;
      },
      (error) => {
        console.error('Error fetching similar title:', error);
      }
    );
  }

  public exchangecurrentSearchValueWithSimilarTitle(): void {
    const service = this.serviceMapping[this.selectedType];

    this.currentSearchValue = this.similarTitle;
    this.fetchContent(service, this.similarTitle); 
    this.fetchDetails(service, this.similarTitle);
    this.similarTitle = '';
  }


  public openDetails(content: SimilarContent) {
    this.dialog.open(DisplaySimilarityDetailsComponent, {
      width: '30%'
      , data: { content }
    });
  }

  @HostListener('wheel', ['$event'])
  onScroll(event: WheelEvent) {
    if (event.deltaY > 0) {
      this.selectedIndex = (this.selectedIndex + 1) % this.similarContent.length;
    } else {
      this.selectedIndex = (this.selectedIndex - 1 + this.similarContent.length) % this.similarContent.length;
    }
  }
}
