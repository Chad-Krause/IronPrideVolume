import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NameService } from 'src/app/services/name.service';
import { RealTimeService } from 'src/app/services/real-time.service';

@Component({
  selector: 'settings-dialog',
  templateUrl: './settings-dialog.component.html',
  styleUrls: ['./settings-dialog.component.scss']
})
export class SettingsDialog implements OnInit {
  name: FormControl;
  clientKey: FormControl;

  constructor(private rt: RealTimeService, private ns: NameService, public dialogRef: MatDialogRef<SettingsDialog>) { 
    this.name = new FormControl(ns.getName(), [Validators.required, Validators.minLength(4), Validators.maxLength(20)]);
    this.clientKey = new FormControl(rt.getClientKey(), [Validators.required, Validators.minLength(6), Validators.maxLength(20)]);
  }

  ngOnInit(): void {
  }

  onSave(): void {
    if(!this.name.valid || !this.clientKey.valid) {
      return;
    }

    this.ns.setName(this.name.value);
    this.rt.setClientKey(this.clientKey.value);
    this.dialogRef.close();
  }

  cancel(): void {
    this.dialogRef.close();
  }

  getErrorMessage(control: FormControl, id: number): string {
    if(control.hasError("required")) {
      if(id == 0) {
        return "Client key is required";
      }
      
      if(id == 1) {
        return "Device name is required";
      }
    }

    if(control.hasError("minlength")) {
      if(id == 0) {
        return "Length must be longer than 6 characters";
      }
      
      if(id == 1) {
        return "Length must be longer than 4 characters";
      }
    }

    if(control.hasError("maxlength")) {
      return "Okay, now it's too long";
    }
  }

}
