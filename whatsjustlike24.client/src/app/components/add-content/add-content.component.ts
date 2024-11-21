import { Component } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { AddContentDialogComponent } from 'src/app/components/add-content-dialog/add-content-dialog.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-add-content',
  templateUrl: './add-content.component.html',
  styleUrls: ['./add-content.component.scss']
})
export class AddContentComponent {
  currentSearchValue: string = '';
  private searchStateSubscription: Subscription;

  constructor(
    public dialog: MatDialog,
    private setSearchValueService: SetSearchValueService
  ) {
    this.searchStateSubscription = this.setSearchValueService.getSearchState().subscribe(
      response => {
        this.currentSearchValue = response.value;
      }
    );
  }

  ngOnDestroy() {
    this.searchStateSubscription.unsubscribe(); // Prevent memory leaks
  }

  openDialog() {
    this.dialog.open(AddContentDialogComponent, {
      data: { currentSearchValue: this.currentSearchValue },
      width: '30%'
    });
  }
}
