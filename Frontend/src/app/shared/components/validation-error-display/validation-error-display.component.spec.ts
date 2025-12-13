import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidationErrorDisplayComponent } from './validation-error-display.component';

describe('ValidationErrorDisplayComponent', () => {
  let component: ValidationErrorDisplayComponent;
  let fixture: ComponentFixture<ValidationErrorDisplayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ValidationErrorDisplayComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ValidationErrorDisplayComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
