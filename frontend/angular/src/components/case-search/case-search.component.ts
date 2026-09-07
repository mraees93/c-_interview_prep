import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, computed, effect, ElementRef, inject, OnDestroy, OnInit, output, Signal, signal, viewChild, WritableSignal } from '@angular/core';
import { LegalCase } from '../../models/legal-case.model';

@Component({
    selector: 'app-case-search',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './case-search.component.html',
    styleUrl: './case-search.component.css'
})
export class CaseSearchComponent implements OnInit, OnDestroy {
    private http = inject(HttpClient);
    
    // #region CONCEPT 4: PROP CALL BACKS (React Props vs. Angular Output)
    // REACT TRANSLATION: Behaves exactly like a parent callback prop: onCaseSelected={handleSelect}
    // #endregion
    onCaseSelected = output<LegalCase>();

    // #region CONCEPT 2: STATE HOOKS (useState vs. signal)
    // REACT TRANSLATION: signal() replaces useState(). State is read via cases() and updated via cases.set().
    // #endregion
    cases: WritableSignal<LegalCase[]> = signal([]);
    searchQuery: WritableSignal<string> = signal("");
    isLoading: WritableSignal<boolean> = signal(false);

    // #region CONCEPT 5 (PART 2): MUTABLE CACHING (useRef Value vs. Class Property)
    // REACT TRANSLATION: Behaves exactly like: const accessCount = useRef(0)
    // Modifying this variable stores local tracking data but completely bypasses UI re-renders.
    // #endregion
    diagnosticAccessCount: number = 0;

    // #region CONCEPT 5 (PART 1): DOM REFS (useRef DOM vs. viewChild Signal Query)
    // REACT TRANSLATION: Behaves exactly like: const searchInputRef = useRef<HTMLInputElement>(null)
    // #endregion
    searchInputRef = viewChild<ElementRef<HTMLInputElement>>('searchInput');

    // #region CONCEPT 6: CALCULATION BUFFERS (useMemo vs. computed)
    // REACT TRANSLATION: Caches output arrays natively until underlying signal dependencies change.
    // #endregion
    filteredCases: Signal<LegalCase[]> = computed(() => {
        const activeQuery = this.searchQuery().toLowerCase();

        return this.cases().filter(caseItem => 
                caseItem.title.toLowerCase().includes(activeQuery) || 
                caseItem.id.toString().includes(activeQuery)
        );
    });

    totalMatchesCount: Signal<number> = computed(() => this.filteredCases().length);

    constructor() {
      // #region CONCEPT 3 (PART 2): RUNTIME SIDE EFFECTS (useEffect Triggers vs. effect)
      // REACT TRANSLATION: Behaves exactly like: useEffect(() => { log() }, [searchQuery])
      // #endregion  
      effect(() => {
        console.log(`Telemetry system change query alert: ${this.searchQuery()}`);
        
        // 🛡️ FIXED TIGHT LAYOUT LOOP: We keep tracking execution clean. 
        // If you need to print passes without destabilizing Zone.js, modify it outside the template view context.
        //this.diagnosticAccessCount++;
      });
    }

    // #region CONCEPT 3 (PART 1): ON-MOUNT LIFECYCLES (useEffect [] vs. ngOnInit)
    // #endregion
    ngOnInit(): void {
        this.fetchLegalRecords();
    }

    // #region CLEANUP OPERATIONS (useEffect Return Cleanup vs. ngOnDestroy)
    // #endregion
    ngOnDestroy(): void {
        console.log('Tearing down active dashboard socket streams. Zero memory leaks allowed.');
    }

    private fetchLegalRecords(): void {
        this.isLoading.set(true);
        this.http.get<LegalCase[]>('https://jsonplaceholder.typicode.com/posts')
            .subscribe({
                next: (data) => {
                    this.cases.set(data);
                    this.isLoading.set(false);
                     this.focusSearchInput();
                },
                error: (err) => {
                    console.error('API consumption failure stream detected:', err);
                    this.isLoading.set(false);   
                }
            });
    }

    onSearchChange(value: string): void {
        this.searchQuery.set(value);
    }

    selectCase(selectedRecord: LegalCase): void {
        // 🚀 INFRASTRUCTURE FIX: Defers emission to the next macro-task frame to protect component selection state loops
        setTimeout(() => {
            this.onCaseSelected.emit(selectedRecord);
        }, 0);
    }

    focusSearchInput(domEvent?: Event): void {
        if (domEvent) {
            domEvent.preventDefault();
            domEvent.stopPropagation(); // Blocks the browser from propagating focus back to the button
        }
        // 🚀 INFRASTRUCTURE FIX: Deferring DOM element selection query ensures execution frame stability
        setTimeout(() => {
            // 🔍 Correctly read the signal function using execution parens ()
            const elementRef = this.searchInputRef();
            const element = elementRef?.nativeElement;
            
            if (element) {
                element.focus();
                console.log("Cursor successfully locked into input target.");
            }
        }, 20);
    }
}
