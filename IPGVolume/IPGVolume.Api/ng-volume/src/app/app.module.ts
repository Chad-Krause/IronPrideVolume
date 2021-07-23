import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { NavBarComponent } from './components/nav-bar/nav-bar.component';
import { ScheduledVolumeChangesComponent } from './components/scheduled-volume-changes/scheduled-volume-changes.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './modules/shared.module';
import { AppRoutingModule } from './app-routing.module';
import { VolumeSliderComponent } from './components/volume-slider/volume-slider.component';
import { SettingsDialog } from './components/modals/settings/settings-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    ScheduledVolumeChangesComponent,
    VolumeSliderComponent,
    SettingsDialog
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    SharedModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
