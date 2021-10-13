import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddEditSvcComponent } from './components/add-edit-svc/add-edit-svc.component';
import { ScheduledVolumeChangesComponent } from './components/scheduled-volume-changes/scheduled-volume-changes.component';


const routes: Routes = [
  {
    path: "schedule",
    component: ScheduledVolumeChangesComponent
  },
  {
    path: "modify-schedule",
    component: AddEditSvcComponent
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
