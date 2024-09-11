import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { AddContentComponent } from './components/add-content/add-content.component';
import { AddContentDialogComponent } from './components/add-content-dialog/add-content-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    AddContentComponent,
    AddContentDialogComponent
  ],
  imports: [
    BrowserModule, HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
