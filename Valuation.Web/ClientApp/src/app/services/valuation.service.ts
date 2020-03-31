import { Injectable, Inject } from '@angular/core';
import { HttpClient} from '@angular/common/http'
import { Observable } from 'rxjs';
import { Summary } from '../models/Valuation-summary';


@Injectable({
  providedIn: 'root'
})
export class ValuationService {
  baseUrl : string;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  getValuationSummary(interval:string) : Observable<Summary>{
    return this.http.get<Summary>(`${this.baseUrl}api/valuation/summary/${interval}`);
  }
}
