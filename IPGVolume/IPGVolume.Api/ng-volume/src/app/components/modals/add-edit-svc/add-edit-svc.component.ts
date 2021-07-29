import { formatDate } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ScheduledVolumeChange } from 'src/app/models/scheduled-volume-change';
import { NameService } from 'src/app/services/name.service';
import { RealTimeService } from 'src/app/services/real-time.service';

@Component({
  selector: 'app-add-edit-svc',
  templateUrl: './add-edit-svc.component.html',
  styleUrls: ['./add-edit-svc.component.scss']
})
export class AddEditSvcComponent implements OnInit {

  svc: ScheduledVolumeChange;
  form: FormGroup;
  action: string = '?';

  constructor(public ns: NameService, private rt: RealTimeService) {
    
    if(this.svc == null) {
      this.svc = new ScheduledVolumeChange(null);
      this.action = 'Add';
    }

    if(this.svc.id) {
      this.action = 'Edit';
    }

    let now: Date = new Date((new Date().getTime()) + 1*60*60*1000); // 2 hours from now for default
    now.setMinutes(0, 0, 0);

    this.form = new FormGroup({
      id: new FormControl(this.svc.id),
      activeOn: new FormControl(now),
      setpoint: new FormControl(0.1),
      creatorName: new FormControl(ns.getName()),
      clientKey: new FormControl(rt.getClientKey()),
      isRecurring: new FormControl(this.svc.isRecurring),
      expiresOn: new FormControl(this.svc.expiresOn)
    });
  }

  ngOnInit(): void {
  }

  cancel(): void {
  }

  onSave(): void {
    
  }

  getUserFriendlyDate(d: Date): string {
    let today: Date = new Date();
    let tomorrow: Date = new Date();
    tomorrow.setDate(today.getDate() + 1);

    if(d.getDate() == today.getDate()) {
      return "Today";
    }

    if(d.getDate() == tomorrow.getDate()) {
      return "Tomorrow"; 
    }

    return formatDate(d, "shortDate", 'en-us')
  }

}
