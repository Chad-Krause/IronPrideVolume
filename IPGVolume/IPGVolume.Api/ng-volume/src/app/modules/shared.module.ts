import { NgModule } from '@angular/core';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/Button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle'
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgbTimepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';


@NgModule({
    imports: [
        MatToolbarModule,
        MatIconModule,
        MatButtonModule,
        MatDialogModule,
        MatFormFieldModule,
        MatMenuModule,
        MatInputModule,
        ReactiveFormsModule,
        MatTableModule,
        MatExpansionModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatCheckboxModule,
        NgbTimepickerModule,
        MatSlideToggleModule,
        FormsModule,
        HttpClientModule
    ],
    exports: [
        MatToolbarModule,
        MatIconModule,
        MatButtonModule,
        MatDialogModule,
        MatFormFieldModule,
        MatMenuModule,
        MatInputModule,
        ReactiveFormsModule,
        MatTableModule,
        MatExpansionModule,
        MatDatepickerModule,
        MatNativeDateModule,
        NgbTimepickerModule,
        MatSlideToggleModule,
        MatCheckboxModule,
        FormsModule,
        HttpClientModule
    ]
  })
export class SharedModule { }