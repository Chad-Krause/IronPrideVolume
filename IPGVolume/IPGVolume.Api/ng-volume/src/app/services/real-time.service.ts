import { EventEmitter, Injectable } from '@angular/core';
import * as SignalR from '@microsoft/signalr'

@Injectable({
  providedIn: 'root'
})
export class RealTimeService {

  private connection: SignalR.HubConnection;
  isConnected: EventEmitter<boolean> = new EventEmitter<boolean>();
  isCurrentlyConnected: boolean = false;

  constructor() {
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


  }

  /**
   * Ensures the right groups are joined on the hub when the service connects/reconnects
   * @param connected True if connected, False if disconnected
   */
  onConnectionChanged(connected: boolean) {
    if (connected) {
    }
  }

  public startConnection() {
    this.connection.start()
      .then(i => this.isConnected.emit(true))
      .catch(i => {
        this.isConnected.emit(false)
      });
  }

}
