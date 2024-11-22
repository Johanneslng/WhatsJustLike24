import { Component } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { AddContentDialogComponent } from 'src/app/components/add-content-dialog/add-content-dialog.component';
import { Subscription } from 'rxjs';
import { ContentType } from 'src/app/models/Enums/ContentType';


@Component({
  selector: 'app-add-content',
  templateUrl: './add-content.component.html',
  styleUrls: ['./add-content.component.scss']
})
export class AddContentComponent {
  currentSearchValue: string = '';
  currentSearchType: ContentType = ContentType.Movies
  private searchStateSubscription: Subscription;

  constructor(
    public dialog: MatDialog,
    private setSearchValueService: SetSearchValueService
  ) {
    this.searchStateSubscription = this.setSearchValueService.getSearchState().subscribe(
      response => {
        this.currentSearchValue = response.value;
        this.currentSearchType = response.type;
      }
    );
  }

  ngOnDestroy() {
    this.searchStateSubscription.unsubscribe(); // Prevent memory leaks
  }

  openDialog() {
    this.dialog.open(AddContentDialogComponent, {
      data: { currentSearchValue: this.currentSearchValue, selectedType: this.currentSearchType },
      width: '30%'
    });
  }

  getEnumLabel(type: ContentType): string {
    return ContentType[type];
  }
}
