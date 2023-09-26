import { Category } from "./category";

export interface Vehicle {
    id: number;
    ownerName: string;
    manufacturer: string;
    yearOfManufacture: number;
    weightKg: number;
    categoryId: number;
    category: Category
}

