import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';

import { CategoryService } from '../../services/category.service';
import { Category } from 'src/app/interfaces/category';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.scss']
})
export class CategoryComponent {
  categoryForm: FormGroup;
  public isUpdate: boolean = false;
  public CATEGORY_LIST = '/category-list';
  public categoryIcons: any = {
    directions_car: false,
    local_shipping: false,
    directions_bus: false,
    directions_railway: false,
    directions_boat: false,
    flight: false
  };

  constructor(private _fb: FormBuilder, 
    private _router:Router, 
    private _route: ActivatedRoute,
    private _snackBar: MatSnackBar,
    private categoryService: CategoryService) {
    this.categoryForm = this._fb.group({
      id: [0],
      name: ['', Validators.required],
      iconUrl: ['', Validators.required],
      minWeightKg: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]],
      maxWeightKg: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]]
    });
  }

  get name() { return this.categoryForm.get('name'); }
  get iconUrl() { return this.categoryForm.get('iconUrl'); }
  get minWeightKg() { return this.categoryForm.get('minWeightKg'); }
  get maxWeightKg() { return this.categoryForm.get('maxWeightKg'); }

  ngAfterViewInit(): void {
    this._route.paramMap.subscribe((params) => {
      const id = Number(params.get("id"));
      if (id) {
        this.isUpdate = true;
        this.getCategoryById(id);
      }
    });
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService
			.get()
			.subscribe(
        {
          next: (categoriesList: Category[]) => {
					this.disableIcons(categoriesList);
				},
				error: (error) => {
					if (error.status === 400) {
            this._snackBar.open(error.error.errors[0], "error")
					 }
				}
      })
  }
  
  disableIcons(categoriesList: Category[]) {
    categoriesList.forEach(element => { this.categoryIcons[element.iconUrl] = true });
  }

  getCategoryById(id: number) {
    this.categoryService.getById(id).subscribe({
      next: (category: Category) =>{
        this.categoryForm.reset(category);
      },
      error: (error) =>{
        this._snackBar.open(error.error.errors[0], "error")
      }
    });
  }
  

  createCategory(newCategory: Category ) {
    this.categoryService.post(newCategory).subscribe({
      next: () =>{
        this._router.navigate([this.CATEGORY_LIST]);
      },
      error: (error) =>{
        this._snackBar.open(error.error.errors[0], "error")
      },
      complete: () => {
        this._snackBar.open("saved", "ok", { panelClass: ['style-success'] })
      }
    });
  }

  updateCategory(category: Category ) {
    this.categoryService.put(category).subscribe({
      next: () =>{
        this._router.navigate([this.CATEGORY_LIST]);
      },
      error: (error) =>{
        this._snackBar.open(error.error.errors[0], "error")
      },
      complete: () => {
        this._snackBar.open("saved", "ok", { panelClass: ['style-success'] })
      }
    });
  }

  onSubmit() {
    if (this.categoryForm.valid) {
      this.isUpdate ?  
        this.updateCategory(this.categoryForm.value):
        this.createCategory(this.categoryForm.value);
    }
  }
}




