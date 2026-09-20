import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommentLog } from '../../models/comment-log.model';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-api-consumption',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './api-consumption.component.html',
    styleUrl: './api-consumption.component.css'
})
export class ApiConsumptionComponent {
    private http = inject(HttpClient);

    //signals (like useState)
    searchTerm = signal<string>('');
    sortDirection = signal<'ASC' | 'DESC'>('ASC');

    comments = signal<CommentLog[]>([]);
    isLoading = signal<boolean>(false);
    error = signal<string | null>(null);

    constructor() {
        //Native effect() (like useEffect(() => {}, []))
        // fires once when component is painted onto the DOM

        // effect(async () => {
        //     try {
        //         this.isLoading.set(true);

        //         const data = await firstValueFrom(
        //             this.http.get<CommentLog[]>('https://jsonplaceholder.typicode.com/comments')
        //         );

        //         this.comments.set(data);
        //     } catch (err: any) {
        //         this.error.set(err.message || 'Failed to populate from API');
        //     } finally {
        //         this.isLoading.set(false);
        //     }
        // })
        effect(async () => {
            try {
                this.isLoading.set(true);

                const data = await firstValueFrom(
                    this.http.get<CommentLog[]>('https://jsonplaceholder.typicode.com/comments')
                );

                this.comments.set(data);
            } catch (err: unknown) {       // using any type turns off type checking, unknown forces you prove what the error type is before you use it, preventing runtime crashes
                
                if(err instanceof Error) {
                    this.error.set(err.message);
                } else if (typeof err === "string") {
                    this.error.set(err || 'Failed to populate from API');
                } else {
                    this.error.set("An unexpected system exception occurred.");
                }
                
            } finally {
                this.isLoading.set(false);
            }
        })
    }

    //high performance sort & filter pipeline (like useMemo)
    processedComments = computed(() => {
        //fallbacks if data is still streaming down from network
        const list = this.comments();
        const search = this.searchTerm().toLowerCase().trim();
        const isAscending = this.sortDirection() === 'ASC';

        let result = list;
        if(search) {
            result = result.filter(item => {
                const nameMatch = item.name ? item.name.toLowerCase().includes(search) : false;
                const emailMatch = item.email ? item.email.toLowerCase().includes(search) : false;
                return nameMatch || emailMatch;
            });
        }

        return [...result].sort((a, b) => {
            return isAscending ? a.id - b.id : b.id - a.id
        });
    })

    handleSearch(event: Event) {
        const input = event.target as HTMLInputElement
        this.searchTerm.set(input.value);
    }

    toggleSort() {
        this.sortDirection.update(current => current === 'ASC' ? 'DESC' : 'ASC');
    }
}
