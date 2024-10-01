import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { LoginComponent } from './components/login/login.component';
import { AddContentComponent } from './components/add-content/add-content.component';
import { AddContentDialogComponent } from './components/add-content-dialog/add-content-dialog.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { SearchComponent } from './components/search/search.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DisplayMoviesComponent } from './components/display-movies/display-movies.component';
import { DisplaySimilarityDetailsComponent } from './components/display-similarity-details/display-similarity-details.component';
import { ScrollingModule } from '@angular/cdk/scrolling'; 
//Angular Material
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({ declarations: [
    AppComponent,
    HeaderComponent,
    LoginComponent,
    AddContentComponent,
    AddContentDialogComponent,
    SearchComponent,
    DisplayMoviesComponent,
    DisplaySimilarityDetailsComponent
    ],
  bootstrap: [AppComponent], imports: [
    BrowserModule,
    //Material
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    FormsModule,
    ScrollingModule,
    MatProgressSpinnerModule,
    BrowserAnimationsModule,

  ], providers: [provideHttpClient(withInterceptorsFromDi()), provideAnimationsAsync('noop')]
})
export class AppModule { }
