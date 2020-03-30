import { TestBed } from '@angular/core/testing';

import { ValuationService } from './valuation.service';

describe('ValuationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ValuationService = TestBed.get(ValuationService);
    expect(service).toBeTruthy();
  });
});
