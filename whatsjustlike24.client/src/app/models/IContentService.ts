import { Observable } from 'rxjs';
import { ContentWithDetails } from 'src/app/models/ContentWithDetails';

export interface IContentService {
  fetchDetails(searchValue: string): Observable<ContentWithDetails>;
  fetchContent(searchValue: string): Observable<any>;
  fetchSimilarTitle(searchValue: string): Observable<any>;
}
