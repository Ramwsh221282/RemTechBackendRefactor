import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FastNavigationSparesComponent } from './fast-navigation-spares.component';

describe('FastNavigationSparesComponent', () => {
  let component: FastNavigationSparesComponent;
  let fixture: ComponentFixture<FastNavigationSparesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FastNavigationSparesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FastNavigationSparesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
