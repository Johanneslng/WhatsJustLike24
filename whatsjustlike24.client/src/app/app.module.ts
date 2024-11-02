import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { LoginComponent } from './components/login/login.component';
import { LogoutComponent } from './components/logout/logout.component';
import { RegisterComponent } from './components/register/register.component';
import { AddContentComponent } from './components/add-content/add-content.component';
import { AddContentDialogComponent } from './components/add-content-dialog/add-content-dialog.component';
import { provideToastr } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { SearchComponent } from './components/search/search.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DisplayMoviesComponent } from './components/display-movies/display-movies.component';
import { DisplaySimilarityDetailsComponent } from './components/display-similarity-details/display-similarity-details.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { ToastrModule } from 'ngx-toastr';
import { FirstKeyPipe } from './pipes/first-key.pipe'

//Angular Material
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon'; 
import { first } from 'rxjs';

@NgModule({ declarations: [
    AppComponent,
    HeaderComponent,
    LoginComponent,
    LogoutComponent,
    RegisterComponent,
    AddContentComponent,
    AddContentDialogComponent,
    SearchComponent,
    DisplayMoviesComponent,
    DisplaySimilarityDetailsComponent,
    FirstKeyPipe,
    ],
  bootstrap: [AppComponent], imports: [
    CommonModule,
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    ScrollingModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    //Material
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatIconModule,

  ], providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimationsAsync('noop'),
    provideToastr({ positionClass: 'toast-top-right', timeOut: 3000 }),
    FirstKeyPipe
  ]
})
export class AppModule { }
