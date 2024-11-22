import { ContentType } from 'src/app/models/Enums/ContentType';

export class ContentTypeSingular {


public contentStringSingular(contentType: ContentType): string {
    switch (contentType) {
      case ContentType.Movies:
        return 'Movie';
      case ContentType.Games:
        return 'Game';
      case ContentType.Shows:
        return 'Show';
      case ContentType.Books:
        return 'Book';
      default:
        return 'Movie';

    }
  }

 public contentStringPlural(contentType: ContentType): string {
    switch (contentType) {
      case ContentType.Movies:
        return 'Movies';
      case ContentType.Games:
        return 'Games';
      case ContentType.Shows:
        return 'Shows';
      case ContentType.Books:
        return 'Books';
      default:
        return 'Movies';

    }
  }
}
