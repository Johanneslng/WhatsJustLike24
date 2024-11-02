import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplaySimilarityDetailsComponent } from './display-similarity-details.component';

describe('DisplaySimilarityDetailsComponent', () => {
  let component: DisplaySimilarityDetailsComponent;
  let fixture: ComponentFixture<DisplaySimilarityDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DisplaySimilarityDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DisplaySimilarityDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
