import { TestBed } from '@angular/core/testing';

import { SetSearchValueService } from './set-search-value.service';

describe('SetSearchValueService', () => {
  let service: SetSearchValueService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SetSearchValueService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
