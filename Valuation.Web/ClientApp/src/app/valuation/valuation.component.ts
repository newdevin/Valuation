import { Component, OnInit,OnDestroy } from '@angular/core';
import { ValuationService } from '../services/valuation.service';
import { ValuationSummary } from '../models/valuation-summary';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-valuation',
  templateUrl: './valuation.component.html',
  styleUrls: ['./valuation.component.css']
})
export class ValuationComponent implements OnInit, OnDestroy {

  subscription: Subscription;
  data : any;
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  valuations : ValuationSummary[]= [];
  constructor(private valuationService:ValuationService) { }

  ngOnInit() {
    this.subscription = this.valuationService.getValuationSummary().subscribe(valuations => {
      this.valuations = valuations;

    });
    this.data = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
          {
              label: 'First Dataset',
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

}
