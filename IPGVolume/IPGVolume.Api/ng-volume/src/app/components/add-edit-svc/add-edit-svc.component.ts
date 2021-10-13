import { formatDate } from '@angular/common';
import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatAccordion } from '@angular/material/expansion';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { ScheduledVolumeChange } from 'src/app/models/scheduled-volume-change';
import { NameService } from 'src/app/services/name.service';
import { RealTimeService } from 'src/app/services/real-time.service';

@Component({
  selector: 'app-add-edit-svc',
  templateUrl: './add-edit-svc.component.html',
  styleUrls: ['./add-edit-svc.component.scss']
})
export class AddEditSvcComponent implements OnInit {
  @ViewChild(MatAccordion) accordion: MatAccordion;

  svc: ScheduledVolumeChange;
  form: FormGroup;
  action: string = '?';
  minDate: Date;
  selectedDateCache: Date;
  time: NgbTimeStruct = { hour: 0, minute: 0, second: 0 };
  calendarDate: Date = new Date();
  recurringCache: boolean = false;
  weekDay: DayOfWeek[] = [
    { name: "Mon", checked: false, dayNumber: 2 },
    { name: "Tue", checked: false, dayNumber: 3 },
    { name: "Wed", checked: false, dayNumber: 4 },
    { name: "Thur", checked: false, dayNumber: 5 },
    { name: "Fri", checked: false, dayNumber: 6 }
  ];
  weekendDay: DayOfWeek[] = [
    { name: "Sat", checked: false, dayNumber: 7 },
    { name: "Sun", checked: false, dayNumber: 1 }
  ]

  destroyableObservables: Subscription[] = [];

  constructor(public ns: NameService, private rt: RealTimeService) {
    this.minDate = new Date();
    

    if(this.svc == null) {
      this.svc = new ScheduledVolumeChange(null);
      this.action = 'Add';
    }

    if(this.svc.id) {
      this.action = 'Edit';
    }

    let now: Date = new Date((new Date().getTime()) + 1*60*60*1000); // 2 hours from now for default
    now.setMinutes(0, 0, 0);

    this.time.hour = now.getHours();

    this.form = new FormGroup({
      id: new FormControl(this.svc.id),
      activeOn: new FormControl(now),
      setpoint: new FormControl(0.1),
      creatorName: new FormControl(ns.getName()),
      clientKey: new FormControl(rt.getClientKey()),
      isRecurring: new FormControl(this.svc.isRecurring),
      expiresOn: new FormControl(this.svc.expiresOn)
    });


    this.selectedDateCache = this.form.get("activeOn").value;
    let x = this.form.get("activeOn").valueChanges.subscribe(newValue => this.selectedDateCache = newValue);
    this.destroyableObservables.push(x);

    this.recurringCache = this.form.get("isRecurring").value;
    let y = this.form.get("isRecurring").valueChanges.subscribe(newValue => {
      this.recurringCache = newValue;
      if(!this.recurringCache) {
        this.accordion.closeAll();
      }
    });
    this.destroyableObservables.push(y);
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.destroyableObservables.forEach(sub => sub.unsubscribe());
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

  getUserFriendlyTime(d: Date): string {
    return formatDate(d, "shortTime", 'en-us')
  }

  getUserFriendlyDays(): string {
    let days: DayOfWeek[] = [];
    days = this.weekDay.concat(this.weekendDay);
    days = days.sort((a, b) => a.dayNumber - b.dayNumber).filter(j => j.checked);

    let dayRepresentation = days.map(i => i.name).join(', ');

    if(dayRepresentation == 'Sun, Sat') {
      return "Weekends";
    }

    if(dayRepresentation == 'Mon, Tue, Wed, Thur, Fri') {
      return "Weekdays"
    }

    if(days.length == 7) {
      return "Every day"
    }
    
    if(days.length > 0 && this.recurringCache) {
      return dayRepresentation;
    }

    return "None";
  }

  newDateSelected(event: Date) {
    let currentDate: Date = this.form.get("activeOn").value;
    currentDate.setDate(event.getDate());

    this.form.get("activeOn").setValue(currentDate);
  }

  newTimeSelected(event: NgbTimeStruct) {
    let currentDate: Date = this.form.get("activeOn").value;
    currentDate.setHours(event.hour, event.minute);
    this.form.get("activeOn").setValue(currentDate);
  }

}

interface DayOfWeek {
  name: string;
  checked: boolean;
  dayNumber: number;
}