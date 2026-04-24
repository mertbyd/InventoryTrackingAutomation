import { Component, OnInit } from '@angular/core';
import { InventoryTrackingAutomationService } from '../services/inventory-tracking-automation.service';

@Component({
  selector: 'lib-inventory-tracking-automation',
  template: ` <p>inventory-tracking-automation works!</p> `,
  styles: [],
})
export class InventoryTrackingAutomationComponent implements OnInit {
  constructor(private service: InventoryTrackingAutomationService) {}

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
