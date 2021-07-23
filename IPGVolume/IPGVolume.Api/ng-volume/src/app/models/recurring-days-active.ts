export class RecurringDaysActive {
    id: number;
    scheduledVolumeChangeId: number;
    dayNumber: number;

    constructor(obj: any) {
        Object.keys(obj).forEach(key => {
            this[key] = obj[key];
        });
    }

    get dayName(): string {
        switch (this.dayNumber) {
            case 1: {
                return "Sunday";
            }
            case 2: {
                return "Monday";
            }
            case 3: {
                return "Tuesday";
            }
            case 4: {
                return "Wednesday";
            }
            case 5: {
                return "Thursday";
            }
            case 6: {
                return "Friday";
            }
            case 7: {
                return "Saturday";
            }

            default: {
                return "Invalid Day!";
            }
        }
    }

    get dayAbbreviation(): string {
        switch (this.dayNumber) {
            case 1: {
                return "Sun";
            }
            case 2: {
                return "M";
            }
            case 3: {
                return "T";
            }
            case 4: {
                return "W";
            }
            case 5: {
                return "Th";
            }
            case 6: {
                return "F";
            }
            case 7: {
                return "Sat";
            }

            default: {
                return "Invalid Day!";
            }
        }
    }
}