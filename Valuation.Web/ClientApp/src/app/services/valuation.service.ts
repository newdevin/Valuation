import { Injectable, Inject } from '@angular/core';
import { HttpClient} from '@angular/common/http'
import { Observable } from 'rxjs';
import { ValuationSummary } from '../models/Valuation-summary';


@Injectable({
  providedIn: 'root'
})
export class ValuationService {
  baseUrl : string;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { 
    this.baseUrl = baseUrl;
  }

  getValuationSummary() : Observable<ValuationSummary[]>{
    return this.http.get<ValuationSummary[]>(`${this.baseUrl}api/valuation/summary`);
  }
}
