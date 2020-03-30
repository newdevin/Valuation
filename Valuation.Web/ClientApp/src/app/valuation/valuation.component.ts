import { Component, OnInit, OnDestroy } from '@angular/core';
import { ValuationService } from '../services/valuation.service';
import { Subscribable, Subscription, from } from 'rxjs';
import { ValuationSummary } from '../models/Valuation-summary';
import * as moment from 'moment';
import { DateService } from '../services/date.service';
import { formatNumber } from '@angular/common';
import { SeriesData } from '../models/series-data';

@Component({
  selector: 'app-valuation',
  templateUrl: './valuation.component.html',
  styleUrls: ['./valuation.component.css']
})
export class ValuationComponent implements OnInit, OnDestroy {

  subscription : Subscription;
  valuationSummaries : ValuationSummary[];
  data:any;
  intervals : string[] = ['1W','1M','3M','6M','1Y', '3Y', '5Y','All'];
  options: any;
  
  constructor(private valuationService: ValuationService, private dateService:DateService) { 

    this.data = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
          {
              label: 'Total Cost',
              data: [65, 59, 80, 81, 56, 55, 40],
              fill: false,
              borderColor: '#4bc0c0'
          },
          {
              label: 'Second Dataset',
              data: [28, 48, 40, 19, 86, 27, 90],
              fill: false,
              borderColor: '#565656'
          }
      ]
  }
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  ngOnInit() {
      this.subscription = this.valuationService.getValuationSummary().subscribe(vals => {
          this.valuationSummaries = vals;
          var series = this.getSeries('3M');
      this.data = {
        labels: series.X,
        datasets : [
          {
            label: 'Total Cost',
            data: series.Y,
            fill : false,
            borderColor : '#565656'
          }
        ],
      } 
      this.options = {
        scales: {
            xAxes: [{
                type: 'time',
                time: {
                  displayFormats: {
                    day: 'DD MMM YY'
                }
                }
            }]
        }
    }
         
      });
  }

  private getSeries( interval : string): SeriesData {
    let fromDate = moment();
    let toDate = fromDate.clone().add(7, 'day');
    let dates :  moment.Moment[];
    let data : number [] = [];
      if(interval === '1W'){
        toDate = fromDate.clone().add(7,'day');
      }
      else if(interval === '1M'){
        toDate = fromDate.clone().add(1, 'month');
      }
      else if(interval === '3M'){
        toDate = fromDate.clone().add(3, 'month');
      }
      else if(interval === '6M'){
        toDate = fromDate.clone().add(6, 'month');
      }
      else if(interval === '1Y'){
        toDate = fromDate.clone().add(1, 'year');
      }
      else if(interval === '3Y'){
        toDate = fromDate.clone().add(3, 'year');
      }
      else if(interval === '5Y'){
        toDate = fromDate.clone().add(5, 'year');
      }
      else{
        toDate = moment(new Date(Math.max.apply(null,this.valuationSummaries.map(s=> s.day))));
      }
      dates = this.dateService.getDateRange(fromDate, toDate).sort()
      dates.forEach(d => {
        let f = d.startOf('day').toDate().toISOString().substring(0,10);
        var x = this.valuationSummaries.find(s => {
          let t = new Date(s.day).toISOString().substring(0,10);
          return t === f;
        });
        if(x !=null)
          data.push(x.totalCostInGbp);
      });
      return { X : dates, Y : data};
  }

}


