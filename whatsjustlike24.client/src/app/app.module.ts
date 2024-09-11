import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { AddContentComponent } from './components/add-content/add-content.component';
import { AddContentDialogComponent } from './components/add-content-dialog/add-content-dialog.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
//Angular Material
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
 

@NgModule({ declarations: [
    AppComponent,
    HeaderComponent,
    AddContentComponent,
    AddContentDialogComponent


    ],
  bootstrap: [AppComponent], imports: [
    BrowserModule,
    //Material
    MatDialogModule

  ], providers: [provideHttpClient(withInterceptorsFromDi()), provideAnimationsAsync('noop')]
})
export class AppModule { }
