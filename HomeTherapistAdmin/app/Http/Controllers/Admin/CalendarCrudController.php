<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\CalendarRequest;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;

/**
 * Class CalendarCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class CalendarCrudController extends CrudController
{
    use \Backpack\CRUD\app\Http\Controllers\Operations\ListOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\CreateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\UpdateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\DeleteOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\ShowOperation;

    /**
     * Configure the CrudPanel object. Apply settings to all operations.
     *
     * @return void
     */
    public function setup()
    {
        CRUD::setModel(\App\Models\Calendar::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/calendar');
        CRUD::setEntityNameStrings('calendar', 'calendars');
        $this->crud->denyAccess(['update', 'delete']);

    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        CRUD::column('date');
        CRUD::column('dt');
        CRUD::column('quarter')
            ->type('string')
            ->label('quarter')
            ->value(function ($entry) {
                switch ($entry->quarter) {
                    case 1:
                        return '春季';
                    case 2:
                        return '夏季';
                    case 3:
                        return '秋季';
                    case 4:
                        return '冬季';
                    default:
                        return '-';
                }
            });
        CRUD::column('month')
            ->type('string')
            ->label('month')
            ->value(function ($entry) {
                return $entry->month . "月";
            });
        CRUD::column('day')
            ->type('string')
            ->label('day')
            ->value(function ($entry) {
                return $entry->week_of_year . "號";
            });
        CRUD::column('day_of_week')
            ->type('string')
            ->label('day_of_week')
            ->value(function ($entry) {
                switch ($entry->day_of_week) {
                    case 1:
                        return '星期一';
                    case 2:
                        return '星期二';
                    case 3:
                        return '星期三';
                    case 4:
                        return '星期四';
                    case 5:
                        return '星期五';
                    case 6:
                        return '星期六';
                    case 7:
                        return '星期日';
                    default:
                        return '-';
                }
            });
        CRUD::column('week_of_year')
            ->type('string')
            ->label('week_of_year')
            ->value(function ($entry) {
                return "第" . $entry->week_of_year . "週";
            });
        CRUD::column('is_weekend')
            ->type('string')
            ->label('is_weekend')
            ->value(function ($entry) {
                return $entry->is_weekend == 1 ? '週末' : '平日';
            });
        CRUD::column('is_holiday')
            ->type('string')
            ->label('is_holiday')
            ->value(function ($entry) {
                return $entry->is_holiday == 1 ? '節慶' : '平日';
            });

        /**
         * Columns can be defined using the fluent syntax or array syntax:
         * - CRUD::column('price')->type('number');
         * - CRUD::addColumn(['name' => 'price', 'type' => 'number']);
         */
    }

    /**
     * Define what happens when the Create operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-create
     * @return void
     */
    protected function setupCreateOperation()
    {
        CRUD::setValidation(CalendarRequest::class);

        CRUD::field('date');
        CRUD::field('dt');
        CRUD::field('year');
        CRUD::column('quarter');
        CRUD::field('month');
        CRUD::field('day');
        CRUD::field('day_of_week');
        CRUD::field('week_of_year');
        CRUD::field('is_weekend');
        CRUD::field('is_holiday');

        /**
         * Fields can be defined using the fluent syntax or array syntax:
         * - CRUD::field('price')->type('number');
         * - CRUD::addField(['name' => 'price', 'type' => 'number']));
         */
    }

    /**
     * Define what happens when the Update operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-update
     * @return void
     */
    protected function setupUpdateOperation()
    {
        $this->setupCreateOperation();
    }

    protected function setupShowOperation()
    {
        CRUD::column('date');
        CRUD::column('dt');
        CRUD::column('year');
        CRUD::column('quarter')
            ->type('string')
            ->label('quarter')
            ->value(function ($entry) {
                switch ($entry->quarter) {
                    case 1:
                        return '春季';
                    case 2:
                        return '夏季';
                    case 3:
                        return '秋季';
                    case 4:
                        return '冬季';
                    default:
                        return '-';
                }
            });
        CRUD::column('month')
            ->type('string')
            ->label('month')
            ->value(function ($entry) {
                return $entry->month . "月";
            });
        CRUD::column('day')
            ->type('string')
            ->label('day')
            ->value(function ($entry) {
                return $entry->week_of_year . "號";
            });
        CRUD::column('day_of_week')
            ->type('string')
            ->label('day_of_week')
            ->value(function ($entry) {
                switch ($entry->day_of_week) {
                    case 1:
                        return '星期一';
                    case 2:
                        return '星期二';
                    case 3:
                        return '星期三';
                    case 4:
                        return '星期四';
                    case 5:
                        return '星期五';
                    case 6:
                        return '星期六';
                    case 7:
                        return '星期日';
                    default:
                        return '-';
                }
            });
        CRUD::column('week_of_year')
            ->type('string')
            ->label('week_of_year')
            ->value(function ($entry) {
                return "第" . $entry->week_of_year . "週";
            });
        CRUD::column('is_weekend')
            ->type('string')
            ->label('is_weekend')
            ->value(function ($entry) {
                return $entry->is_weekend == 1 ? '週末' : '平日';
            });
        CRUD::column('is_holiday')
            ->type('string')
            ->label('is_holiday')
            ->value(function ($entry) {
                return $entry->is_holiday == 1 ? '節慶' : '平日';
            });
    }
}
