import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VehicleComponent } from './components/vehicle/vehicle.component';
import { CategoryComponent } from './components/category/category.component';
import { CategoryListComponent } from './components/category-list/category-list.component';
import { VehicleListComponent } from './components/vehicle-list/vehicle-list.component';


const routes: Routes = [
  { path: 'vehicle', component: VehicleComponent },
  { path: "vehicle/modify/:id", component: VehicleComponent },
  { path: 'vehicle-list', component: VehicleListComponent },
  { path: 'category', component: CategoryComponent },
  { path: "category/modify/:id", component: CategoryComponent },
  { path: 'category-list', component: CategoryListComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
