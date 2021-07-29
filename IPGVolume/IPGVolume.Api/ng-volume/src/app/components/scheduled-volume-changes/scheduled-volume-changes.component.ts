import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScheduledVolumeChange } from 'src/app/models/scheduled-volume-change';
import { SchedulingService } from 'src/app/services/scheduling.service';

@Component({
  selector: 'app-scheduled-volume-changes',
  templateUrl: './scheduled-volume-changes.component.html',
  styleUrls: ['./scheduled-volume-changes.component.css']
})
export class ScheduledVolumeChangesComponent implements OnInit {
  schedule: ScheduledVolumeChange[] = [];
  oneOffColumns: string[] = ['ExecOn', 'Setpoint', 'Actions'];
  recurringColumns: string[] = ['ExecOn', 'Setpoint', 'Days', 'Actions'];

  constructor(private api: SchedulingService, private router: Router) { }

  ngOnInit(): void {
    this.api.getScheduledVolumeChanges().subscribe(res => {
      this.schedule = res.map(i => new ScheduledVolumeChange(i));
      console.log(this.schedule);
    });
  }

  addOneOff() {
    this.router.navigate(['modify-schedule'])
  }

  edit() {

  }

  deleteSvc() {

  }

}
