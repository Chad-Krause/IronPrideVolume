import { EventEmitter, Injectable } from '@angular/core';
import * as SignalR from '@microsoft/signalr'
import { environment } from 'src/environments/environment';
import { NameService } from './name.service';

@Injectable({
  providedIn: 'root'
})
export class RealTimeService {

  private connection: SignalR.HubConnection;
  isConnected: EventEmitter<boolean> = new EventEmitter<boolean>();
  isCurrentlyConnected: boolean = false;
  reportedVolume: EventEmitter<number> = new EventEmitter<number>();
  private clientKey: string = "";

  private CLIENTKEY: string = "ClientKey";

  constructor(private nameService: NameService) {
    this.clientKey = this.getClientKey();

    this.isConnected.subscribe(res => {
      this.isCurrentlyConnected = res;
      this.onConnectionChanged(res);
    });

    // Timer function to see if the service needs reconnecting
    setInterval(() => {
      if (!this.isCurrentlyConnected) {
        this.startConnection();
      }
    }, 15000)

    this.isConnected.emit(false);

    try {
      this.connection = new SignalR.HubConnectionBuilder()
                                    .withUrl(environment.baseUrl + "AudioHub")
                                    .withAutomaticReconnect()
                                    .configureLogging(SignalR.LogLevel.Warning)
                                    .build();

      this.connection.on("ReportVolumeLevel", (level: number) => {
        console.log(`Client reported volume of: ${level * 100}%`)
        this.reportedVolume.emit(level);
      });

      this.startConnection();

    } catch (e) {
      console.error(e);
    }


  }

  /**
   * Ensures the right groups are joined on the hub when the service connects/reconnects
   * @param connected True if connected, False if disconnected
   */
  private onConnectionChanged(connected: boolean) {
    if (connected) {
      this.subscribeToVolumeReports();
    }
  }

  private startConnection() {
    // this.connection.start()
    //   .then(i => this.isConnected.emit(true))
    //   .catch(i => {
    //     this.isConnected.emit(false)
    //   });
  }

  public getClientKey(): string {
    return localStorage.getItem(this.CLIENTKEY);
  }

  public setClientKey(newKey: string) {
    this.unsubscribeFromVolumeReports();

    localStorage.setItem(this.CLIENTKEY, newKey);
    this.clientKey = newKey;

    this.subscribeToVolumeReports();
  }

  public setVolume(setpoint: number) {
    this.connection.send("SetVolume", this.clientKey, setpoint);
  }

  private subscribeToVolumeReports() {
    this.connection.send("SubscribeToVolumeReports", this.nameService.getName(), this.clientKey);
  }

  private unsubscribeFromVolumeReports() {
    this.connection.send("UnsubscribeFromVolumeReports", this.nameService.getName(), this.clientKey);
  }

}
