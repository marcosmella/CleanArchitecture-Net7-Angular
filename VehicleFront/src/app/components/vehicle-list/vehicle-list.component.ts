import { Component, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { Vehicle } from 'src/app/interfaces/vehicle';
import { VehicleService } from 'src/app/services/vehicle.service';

@Component({
  selector: 'app-vehicle-list',
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.scss']
})
export class VehicleListComponent {
  public displayedColumns: string[] = ["id", "ownerName", "manufacturer", "yearOfManufacture", "weightKg", "categoryId", "options"];
	public vehicleDataSource: MatTableDataSource<Vehicle>;

  @ViewChild('vehicleSort') vehicleSort = new MatSort();
  
  constructor(
    private router: Router,
    private _snackBar: MatSnackBar,
    private vehicleService: VehicleService,
  ){
    this.vehicleDataSource = new MatTableDataSource<Vehicle>();
  }
  ngAfterViewInit() {    
    this.loadVehicles();
  }

  loadVehicles() {
    this.vehicleService
      .get()
      .subscribe(
        {
          next: (vehicles: Vehicle[]) => {
          this.vehicleDataSource.data = vehicles;
          this.vehicleDataSource.sort = this.vehicleSort;
        },
        error: (error) => {
          if (error.status === 400) {
            this._snackBar.open(error.error.errors[0], "error")
          } else {
          	this.vehicleDataSource.data = [];
          }
        }
      })
  }

  editCategory(id: number): void {
		this.router.navigateByUrl(`vehicle/modify/${id}`);
	}

  deleteCategory(id: number) {
    this.vehicleService
    .delete(id)
    .subscribe(
      {
        next: () => {
        this.loadVehicles();
      },
      error: (error) => {
        if (error.status === 400) {
          this._snackBar.open(error.error.errors[0], "error")
        } else {
          this.vehicleDataSource.data = [];
        }
      },
      complete: () => {
        this._snackBar.open("deleted", "ok", { panelClass: ['style-success'] });
      }
    })
  }
}

