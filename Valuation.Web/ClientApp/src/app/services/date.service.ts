import { Injectable } from '@angular/core';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class DateService {

  constructor() { }

  getDateRange(fromDate:moment.Moment, toDate : moment.Moment): moment.Moment[] {

    let minDate = fromDate;
    let maxDate = toDate;
    if(fromDate > toDate)
      minDate = toDate;
    if(toDate < fromDate)
      maxDate = fromDate;

    let curDate = minDate;
    var dates : moment.Moment[] =[];
    while(curDate <= maxDate){
        dates.push(curDate.clone());
        curDate = curDate.add(1,'day');
    }
    return dates;
  }
}
