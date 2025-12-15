import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesDialogComponent } from './roles-dialog.component';

describe('CustomDialogComponent', () => {
  let component: RolesDialogComponent;
  let fixture: ComponentFixture<RolesDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolesDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RolesDialogComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
