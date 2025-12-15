import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateUserRoleDialogComponent } from './update-user-role-dialog.component';

describe('UpdateUserRoleDialogComponent', () => {
  let component: UpdateUserRoleDialogComponent;
  let fixture: ComponentFixture<UpdateUserRoleDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateUserRoleDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateUserRoleDialogComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
