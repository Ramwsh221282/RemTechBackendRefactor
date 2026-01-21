import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FastNavigationVehiclesComponent } from './fast-navigation-vehicles.component';

describe('FastNavigationVehiclesComponent', () => {
  let component: FastNavigationVehiclesComponent;
  let fixture: ComponentFixture<FastNavigationVehiclesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FastNavigationVehiclesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FastNavigationVehiclesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
