import { Component, OnInit } from '@angular/core';
import { ValuationService } from '../services/valuation.service';
import { Subscribable, Subscription, from } from 'rxjs';
import { Summary } from '../models/Valuation-summary';
import * as moment from 'moment';
import { DateService } from '../services/date.service';
import { formatNumber } from '@angular/common';
import { SeriesData } from '../models/series-data';

@Component({
  selector: 'app-valuation-summary',
  templateUrl: './valuation-summary.component.html',
  styleUrls: ['./valuation-summary.component.css']
})
export class ValuationSummaryComponent implements OnInit {

  summary: Summary;
  data: any;
  intervals: string[] = ['1W', '1M', '3M', '6M', '1Y', '3Y', '5Y', 'All'];
  selectedInterval: string = '1M';
  options = {
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

  constructor(private valuationService: ValuationService, private dateService: DateService) {

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

  private buildData() {
    let dates = this.summary.timeSeries;
    this.data = {
      labels: dates,
      datasets: [
        {
          label: 'Buy',
          data: this.summary.cost,
          fill: false,
          borderColor: 'red'
        },
        {
          label: 'Sold',
          data: this.summary.sold,
          fill: false,
          borderColor: 'silver'
        },
        {
          label: 'Valuation',
          data: this.summary.valuation,
          fill: false,
          borderColor: 'blue'
        },
        {
          label: 'Profit',
          data: this.summary.profit,
          fill: false,
          borderColor: 'green'
        }
      ]
    }
  }

  ngOnInit() {
    this.getData();
  }

  async getData(){
    this.summary = await  this.valuationService.getValuationSummary(this.selectedInterval).toPromise();
    this.buildData();
  }

  onSelectionChange(interval) {
    this.selectedInterval = interval;
    this.getData();
  }
}


