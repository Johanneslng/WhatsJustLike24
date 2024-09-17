import {
  Component,
  OnInit,
  OnDestroy
} from '@angular/core';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { GetMoviesService } from 'src/app/services/apiCalls/get-movies.service';
import { SimilarMovie } from 'src/app/models/SimilarMovie';
import { MovieWithDetails } from 'src/app/models/MovieWithDetails';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { Subscription } from 'rxjs';
import { DisplaySimilarityDetailsComponent } from '../display-similarity-details/display-similarity-details.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-display-movies',
  templateUrl: './display-movies.component.html',
  styleUrl: './display-movies.component.scss'
})
export class DisplayMoviesComponent implements OnInit, OnDestroy {

  displayedColumns: string[] = ['thumbnail', 'title', 'averageSimilarityScore', 'ratingCount'];
  similarMovies: SimilarMovie[] = [];

  similarTitle: string = '';
  imageUrl: string = 'https://image.tmdb.org/t/p/w500/';
  currentSearchValue: string = '';
  isLoading: boolean = false;
  hasSearched: boolean = false;

  hoveredMovie: SimilarMovie = {
    title: '',
    averageSimilarityScore: 0,
    similarityScoreCount: 0,
    posterPath: '',
    descriptionList: '',
    similarityScoreList: ''
  };
  searchMovie: MovieWithDetails = {
    title: '',
    genre: '',
    director: '',
    posterPath: '',
    description: '',
  };

  selectedIndex: number = 0;

  private searchValueSubscription?: Subscription;

  constructor(
    private getMoviesService: GetMoviesService
    , private setSearchValueService: SetSearchValueService
    , public dialog: MatDialog
  ) { }

  ngOnInit() {
    this.searchValueSubscription = this.setSearchValueService.getSearchValue().subscribe(
      (newSearchValue: string) => {
        if (newSearchValue && newSearchValue.length > 0) {
          this.fetchSearchMovieDetails(newSearchValue);
          this.fetchMovies(newSearchValue);
          this.fetchSimilarTitle(newSearchValue);
          this.currentSearchValue = newSearchValue;
          this.hasSearched = true;
        }
      }
    );

  }

  ngOnDestroy() {
    // Don't forget to unsubscribe to prevent memory leaks
    this.searchValueSubscription?.unsubscribe();
  }

  private fetchSearchMovieDetails(searchValue: string) {
    this.searchMovie = {
      title: '',
      genre: '',
      director: '',
      posterPath: '',
      description: '',
    };
    this.getMoviesService
      .getMovieWithDetails(searchValue)
      .subscribe((response: MovieWithDetails) => {
        this.searchMovie = {
          ...response,
          posterPath: this.imageUrl + response.posterPath
        };
      }, error => {
        console.error(error);
      });
  }

  private fetchMovies(searchValue: string) {
    this.isLoading = true;
    this.similarMovies = [];

    this.getMoviesService
      .getSimilarMoviesByTitle(searchValue)
      .subscribe((response: SimilarMovie[]) => {
        this.similarMovies = response.map(movie => ({
          ...movie,
          posterPath: this.imageUrl + movie.posterPath // Concatenate imageUrl with posterPath
        }));
        this.isLoading = false;
      }, error => {
        console.error(error);
        this.isLoading = false;
      });
  }

  private fetchSimilarTitle(searchValue: string) {

    this.getMoviesService
      .getSimilarTitle(searchValue)
      .subscribe((response: string) => {
        this.similarTitle = response;
        console.log(this.similarTitle);
      }, error => {
        console.error(error);
      });
  }

  public exchangecurrentSearchValueWithSimilarTitle() {
    this.currentSearchValue = this.similarTitle;
    this.fetchMovies(this.similarTitle);
    this.similarTitle = '';

  }

  public openMovieDetails(movie: SimilarMovie) {
    this.dialog.open(DisplaySimilarityDetailsComponent, {
      width: '30%'
      , data: { movie: movie }
    });
  }

  public onClick() {
    this.selectedIndex += 1;
  }

  public onClick2() {
    if (this.selectedIndex > 0) {
      this.selectedIndex -= 1;
    }
  }
}
