export class FilterParams {
    public filterName: string = 'Filter';
    public nodes: any[] = [];
    public defaultSelection?: any[] = [];
    public initialSelection?: any[] = [];
    public anyOrAll?: 'Any' | 'All'; //if defined, add a row at top for selecting any or all
    public checkMode?: 'single' | 'multiple';
}