import { RecurringDaysActive } from "./recurring-days-active";

export class ScheduledVolumeChange {
    id?: number;
    setpoint: number;
    creatorName: string;
    clientKey: string;
    isRecurring: boolean;
    activeOn: Date;
    createdOn: Date;
    expiresOn?: Date;
    completedOn?: Date;

    recurringDaysActive: RecurringDaysActive[];

    constructor(obj: any) {
        Object.keys(obj).forEach(key => {
            this[key] = obj[key];
        });

        this.activeOn = new Date(this.activeOn);
        this.createdOn = new Date(this.createdOn);
        
        if(this.expiresOn) {
            this.expiresOn = new Date(this.expiresOn);
        }
        
        if(this.completedOn) {
            this.completedOn = new Date(this.completedOn);
        }

        this.recurringDaysActive = this.recurringDaysActive.map(i => new RecurringDaysActive(i));
    }

    get nice_setpoint(): string {
        return `${this.setpoint * 100}%`;
    }
}