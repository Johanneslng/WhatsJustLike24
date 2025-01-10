import {
  Component,
  OnInit,
  OnDestroy,
  HostListener,
  ElementRef,
  ViewChild
} from '@angular/core';
import { GetMoviesService } from 'src/app/services/apiCalls/get-movies.service';
import { GetGamesService } from 'src/app/services/apiCalls/get-games.service';
import { GetShowsService } from 'src/app/services/apiCalls/get-shows.service';
import { SimilarMovie } from 'src/app/models/SimilarMovie';
import { SimilarContent } from 'src/app/models/SimilarContent';
//import { MovieWithDetails } from 'src/app/models/MovieWithDetails';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { ContentType } from 'src/app/models/Enums/ContentType';
import { Subscription, BehaviorSubject, combineLatest, catchError, of } from 'rxjs';
import { distinctUntilChanged, switchMap } from 'rxjs/operators';
import { DisplaySimilarityDetailsComponent } from '../display-similarity-details/display-similarity-details.component';
import { MatDialog } from '@angular/material/dialog';
import { IContentService } from 'src/app/models/IContentService';
import { ContentWithDetails, GameWithDetails, MovieWithDetails } from 'src/app/models/ContentWithDetails';
import { ContentTypeSingular } from 'src/app/helpers/ContentTypeSingular';
import { GetBooksService } from 'src/app/services/apiCalls/get-books.service';


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

  private contentTypeSingular: ContentTypeSingular = new ContentTypeSingular();

  constructor(
    private getMoviesService: GetMoviesService,
    private getGamesService: GetGamesService,
    private getShowsService: GetShowsService,
    private getBooksService: GetBooksService,
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
      [ContentType.Shows]: {
        fetchDetails: (searchValue: string) => this.fetchDetails(this.serviceMapping[ContentType.Shows], searchValue),
        fetchContent: (searchValue: string) => this.fetchContent(this.serviceMapping[ContentType.Shows], searchValue),
        fetchSimilarTitle: (searchValue: string) => this.fetchSimilarTitle(this.serviceMapping[ContentType.Shows], searchValue),
      },
      [ContentType.Books]: { 
        fetchDetails: (searchValue: string) => this.fetchDetails(this.serviceMapping[ContentType.Books], searchValue),
        fetchContent: (searchValue: string) => this.fetchContent(this.serviceMapping[ContentType.Books], searchValue),
        fetchSimilarTitle: (searchValue: string) => this.fetchSimilarTitle(this.serviceMapping[ContentType.Books], searchValue),
      },
    };

    this.serviceMapping = {
      [ContentType.Movies]: this.getMoviesService,
      [ContentType.Games]: this.getGamesService,
      [ContentType.Shows]: this.getShowsService,
      [ContentType.Books]: this.getBooksService
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
            this.hasSearched = false;
            const service = this.serviceMapping[type];
            return combineLatest([
              service.fetchDetails(value).pipe(catchError((err) => this.handleError(err, 'details'))),
              service.fetchContent(value).pipe(catchError((err) => this.handleError(err, 'content'))),
              service.fetchSimilarTitle(value).pipe(catchError((err) => this.handleError(err, 'similar title'))),
            ]);
          })
        )
        .subscribe(
          ([details, content, similarTitle]: [ContentWithDetails | null, SimilarContent[], string]) => {
            if (details) {
              this.searchContent = this.processDetails(details);
            } else {
              console.warn('Details are null');
              this.searchContent = null; // Or handle as needed
            }
            this.similarContent = this.processSimilarContent(content, this.searchState$.value.type);
            this.similarTitle = similarTitle;
            this.isLoading = false;
            this.hasSearched = true;
          },
          (error) => {
            console.error('Unhandled error:', error);
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
      case ContentType.Shows:
        return 'shows/';
      case ContentType.Books:
        return 'books/';
      default:
        return '';
    }
  }

  public getContentTypePlural(): string {
    return this.contentTypeSingular.contentStringPlural(this.searchState$.value.type)
  }


  private handleError(error: any, context: string) {
    console.error(`Error fetching ${context}:`, error);
    this.isLoading = false; // Ensure loading state resets
    return of(null); // Return a fallback value to keep the stream alive
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

          case 'Show':
            this.searchContent = {
              ...response,
              image: this.imageUrl + 'shows/' + response.image
            };
            console.log('Processed show details:', this.searchContent);
            break;

          case 'Book':
            this.searchContent = {
              ...response,
              image: this.imageUrl + 'books/' + response.image
            };
            console.log('Processed book details:', this.searchContent);
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

          case ContentType.Shows:
            this.similarContent = response.map(content => ({
              ...content,
              image: this.imageUrl + 'shows/' + content.image

            }));
            break;

          case ContentType.Books:
            this.similarContent = response.map(content => ({
              ...content,
              image: this.imageUrl + 'books/' + content.image

            }));
            break;

          default:
            console.error('Unknown content type:', response);
            break;
        }
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
    if (!this.similarTitle) {
      console.warn('No similar title to exchange with');
      return;
    }

    this.searchState$.next({
      value: this.similarTitle,
      type: this.searchState$.value.type
    });

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
