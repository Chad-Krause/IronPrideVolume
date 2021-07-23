import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ScheduledVolumeChangesComponent } from './components/scheduled-volume-changes/scheduled-volume-changes.component';


const routes: Routes = [
  {
    path: "schedule",
    component: ScheduledVolumeChangesComponent
  },
  {
    path: "**",
    redirectTo: "/",
    pathMatch: "full"
  },
  {
    path: "",
    redirectTo: "/",
    pathMatch: "full"
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
