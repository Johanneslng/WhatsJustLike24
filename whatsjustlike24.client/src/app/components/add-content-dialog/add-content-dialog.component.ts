import { Component, Inject } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PostMoviesService } from 'src/app/services/apiCalls/post-movies.service';
import { PostGamesService } from 'src/app/services/apiCalls/post-games.service';
import { LoginComponent } from 'src/app/components/login/login.component';
import { UserService } from 'src/app/services/apiCalls/user.service';
import { ContentRelation } from 'src/app/models/ContentRelation';
import { ContentType } from 'src/app/models/Enums/ContentType';
import { IPostContentService } from 'src/app/models/IPostContentService';


@Component({
  selector: 'app-add-content-dialog',
  templateUrl: './add-content-dialog.component.html',
  styleUrls: ['./add-content-dialog.component.scss']
})
export class AddContentDialogComponent {

  private serviceMapping: { [key in ContentType]: IPostContentService };

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private postMoviesService: PostMoviesService,
    private postGamesService: PostGamesService,
    private userService: UserService,
    public dialog: MatDialog,
  ) {
    this.serviceMapping = {
      [ContentType.Movies]: this.postMoviesService,
      [ContentType.Games]: this.postGamesService,
      [ContentType.Shows]: this.postGamesService,
      [ContentType.Books]: this.postGamesService,
    };

  }

  contentRelation: ContentRelation = {
    titleA: '',
    titleB: this.data.currentSearchValue,
    similarityScore: undefined,
    description: ''
  };


  addContent(): void {
    const selectedService = this.serviceMapping[this.data.selectedType]; // Dynamically select the service
    selectedService.addContent(this.contentRelation).subscribe(
      response => {
        console.log('Content added successfully!', response);
      },
      error => {
        console.error('Error adding content!', error);
      }
    );
  }


  openLogin() {
    this.dialog.open(LoginComponent, {
      width: '30%'
    });
  }

  isLoggedIn(): boolean {
    return true; //TODO return this.userService.isLoggedIn();
  }

  getEnumLabel(type: ContentType): string {
    return ContentType[type];
  }
}
