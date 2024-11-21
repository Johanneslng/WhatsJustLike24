import { TestBed } from '@angular/core/testing';

import { PostGamesService } from './post-games.service';

describe('PostGamesService', () => {
  let service: PostGamesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PostGamesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
