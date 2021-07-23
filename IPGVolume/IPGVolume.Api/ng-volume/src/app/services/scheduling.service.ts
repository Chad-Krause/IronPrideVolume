import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ScheduledVolumeChange } from '../models/scheduled-volume-change';
import { RealTimeService } from './real-time.service';

@Injectable({
  providedIn: 'root'
})
export class SchedulingService {
  baseUrl: string;
  constructor(private http: HttpClient, private rt: RealTimeService) { 
    this.baseUrl = environment.baseUrl + "ScheduledVolumeChanges";
  }

  getScheduledVolumeChanges(): Observable<ScheduledVolumeChange[]> {
    return this.http.get<ScheduledVolumeChange[]>(`${this.baseUrl}/GetScheduledVolumeChanges`, { params: { clientKey: this.rt.getClientKey() } });
  }
}
