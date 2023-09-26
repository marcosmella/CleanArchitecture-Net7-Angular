import { Component, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';

import { Category } from 'src/app/interfaces/category';
import { CategoryService } from 'src/app/services/category.service';

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss']
})
export class CategoryListComponent {
  public displayedColumns: string[] = ["id", "name", "iconUrl", "minWeightKg", "maxWeightKg", "options"];
	public categoryDataSource: MatTableDataSource<Category>;

  @ViewChild('categorySort') categorySort = new MatSort();

  constructor(
    private router: Router,
    private _snackBar: MatSnackBar,
		private categoryService: CategoryService
  ){
    this.categoryDataSource = new MatTableDataSource<Category>();
  }

  ngAfterViewInit() {    
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService
			.get()
			.subscribe(
        {
          next: (categories) => {
					this.categoryDataSource.data = categories;
					this.categoryDataSource.sort = this.categorySort;
				},
				error: (error) => {
          if (error.status === 400) {
            this._snackBar.open(error.error.errors[0], "error")
          } else {
						this.categoryDataSource.data = [];
					}
				}
      })
  }

  editCategory(id: number): void {
		this.router.navigateByUrl(`category/modify/${id}`);
	}

  deleteCategory(id: number) {
    this.categoryService
    .delete(id)
    .subscribe(
      {
        next: () => {
        this.loadCategories();
      },
      error: (error) => {
        if (error.status === 400) {
          this._snackBar.open(error.error.errors[0], "error")
        } else {
          this.categoryDataSource.data = [];
        }
      },
      complete: () => {
        this._snackBar.open("deleted", "ok", { panelClass: ['style-success'] });
      }
    })
  }
}
