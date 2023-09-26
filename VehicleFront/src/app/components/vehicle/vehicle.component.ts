import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VehicleService } from '../../services/vehicle.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Vehicle } from 'src/app/interfaces/vehicle';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.scss']
})
export class VehicleComponent {
  vehicleForm: FormGroup;
  public isUpdate: boolean = false;
  public VEHICLE_LIST = '/vehicle-list';

  constructor(private fb: FormBuilder, 
    private router: Router, 
    private route: ActivatedRoute,
    private _snackBar: MatSnackBar,
    private vehicleService: VehicleService) {
    this.vehicleForm = this.fb.group({
      id: [0],
      ownerName: ['', Validators.required],
      manufacturer: ['', Validators.required],
      yearOfManufacture: ['', Validators.required],
      weightKg: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]]
    });
  }

  get ownerName() { return this.vehicleForm.get('ownerName'); }
  get manufacturer() { return this.vehicleForm.get('manufacturer'); }
  get yearOfManufacture() { return this.vehicleForm.get('yearOfManufacture'); }
  get weightKg() { return this.vehicleForm.get('weightKg'); }

  ngAfterViewInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get("id"));
      if (id) {
        this.isUpdate = true;
        this.getVehicleById(id);
      }
    });
  }

  getVehicleById(id: number) {
    this.vehicleService.getById(id).subscribe({
      next: (vehicle: Vehicle) =>{
        this.vehicleForm.reset(vehicle);
      },
      error: (error) =>{
        if(error.status === 400) {
        this._snackBar.open(error.error.errors[0], "error")
        }
      }
    });
  }

  createCategory(newVehicle: Vehicle ) {
    this.vehicleService.post(newVehicle).subscribe({
      next: () =>{
        this.router.navigate([this.VEHICLE_LIST]);
      },
      error: (error) =>{
        this._snackBar.open(error.error.errors[0], "error");
      },
      complete: () => {
        this._snackBar.open("saved", "ok", { panelClass: ['style-success'] });
      }
    });
  }

  updateCategory(vehicle: Vehicle ) {
    this.vehicleService.put(vehicle).subscribe({
      next: () =>{
        this.router.navigate([this.VEHICLE_LIST]);
      },
      error: (error) =>{
        this._snackBar.open(error.error.errors[0], "error");
      },
      complete: () => {
        this._snackBar.open("saved", "ok", { panelClass: ['style-success'] });
      }
    });
  }

  onSubmit() {
    if (this.vehicleForm.valid) {
      this.isUpdate ?  
      this.updateCategory(this.vehicleForm.value):
      this.createCategory(this.vehicleForm.value);
    }
  }
}
