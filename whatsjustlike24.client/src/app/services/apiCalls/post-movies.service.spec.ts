import { TestBed } from '@angular/core/testing';

import { PostMoviesService } from './post-movies.service';

describe('PostMoviesService', () => {
  let service: PostMoviesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PostMoviesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
