import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NameService {
  name: string = "";
  private REMOTE_NAME = "RemoteName";

  constructor() { 
    this.name = localStorage.getItem(this.REMOTE_NAME);
  }

  getName(): string {
    return this.name;
  }
  
  setName(name: string) {
    this.name = name;
    localStorage.setItem(this.REMOTE_NAME, this.name);
  }

  isNameSet(): boolean {
    return this.name.length > 0;
  }
}
