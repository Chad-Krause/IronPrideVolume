import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledVolumeChangesComponent } from './scheduled-volume-changes.component';

describe('ScheduledVolumeChangesComponent', () => {
  let component: ScheduledVolumeChangesComponent;
  let fixture: ComponentFixture<ScheduledVolumeChangesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduledVolumeChangesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduledVolumeChangesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
