import { TestBed } from '@angular/core/testing';

import { PostBooksService } from './post-books.service';

describe('PostBooksService', () => {
  let service: PostBooksService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PostBooksService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
