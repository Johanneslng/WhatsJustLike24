import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SimilarMovie } from 'src/app/models/SimilarMovie';

@Component({
  selector: 'app-display-similarity-details',
  templateUrl: './display-similarity-details.component.html',
  styleUrl: './display-similarity-details.component.scss'
})
export class DisplaySimilarityDetailsComponent {
  constructor(
    public dialogRef: MatDialogRef<DisplaySimilarityDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { movie: SimilarMovie }
  ) { }

  movie: SimilarMovie = this.data.movie;
  descriptions: string[] = this.movie.descriptionList.split(";");
  similarityScores: string[] = this.movie.similarityScoreList.split(";");

  // Example method to close the dialog
  closeDialog(): void {
    this.dialogRef.close();
  }
}
