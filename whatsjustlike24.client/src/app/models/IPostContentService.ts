import { Observable } from 'rxjs';
import { ContentRelation } from 'src/app/models/ContentRelation';
export interface IPostContentService {
  addContent(contentRelation: ContentRelation): Observable<ContentRelation>;
}
