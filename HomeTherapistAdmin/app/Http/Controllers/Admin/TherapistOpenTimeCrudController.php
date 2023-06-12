<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\TherapistOpenTimeRequest;
use App\Models\TherapistOpenTime;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;
use Backpack\CRUD\app\Library\Widget;
use Carbon\Carbon;

/**
 * Class TherapistOpenTimeCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class TherapistOpenTimeCrudController extends CrudController
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
        CRUD::setModel(\App\Models\TherapistOpenTime::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/therapist-open-time');
        CRUD::setEntityNameStrings('therapist open time', 'therapist open times');
        $this->crud->denyAccess(['create', 'update', 'delete']);
        // $currentMonth = Carbon::now()->month;
        // $currentYear = Carbon::now()->year;

        // $this->crud->addClause('whereMonth', 'start_dt', $currentMonth);
        // $this->crud->addClause('whereYear', 'start_dt', $currentYear);

        $this->crud->orderBy('start_dt', 'asc');
        $this->crud->orderBy('user_id', 'asc');
        $this->crud->enablePersistentTable();
        $this->crud->setOperationSetting('persistentTableDuration', 120);
    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        CRUD::setOperationSetting('showEntryCount', false);

        $currentDay = Carbon::now()->day;
        $userCountDay = \App\Models\TherapistOpenTime::whereDay('start_dt', $currentDay)->distinct()->get('user_id')->count();

        Widget::add()
            ->to('before_content')
            ->type('div')
            ->class('row')
            ->content([
                Widget::make([
                    'type' => 'card',
                    'class' => 'card bg-dark text-white',
                    'wrapper' => ['class' => 'col-sm-3 col-md-3'],
                    'content' => [
                        'header' => '本日開放預約的治療師',
                        'body' => '共有' . $userCountDay . ' 位',
                    ],
                ]),
            ]);

        CRUD::column('user_id')
            ->type('relationship')
            ->attribute('user.username')
            ->label('User')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    $user = \App\Models\User::where('staff_id', $entry->user_id)->first();
                    if ($user) {
                        return backpack_url('user/' . $user->id . '/show');
                    }
                    return backpack_url('user/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                $user = \App\Models\User::where('staff_id', $entry->user_id)->first();
                return $user->username ?? '-';
            });

        CRUD::column('start_dt')
            ->type('relationship')
            ->attribute('calendar.Dt')
            ->label('Calendar')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    // dump($entry->start_dt);
                    $date_time = \App\Models\Calendar::where('Dt', $entry->start_dt)->first();
                    if ($date_time) {
                        // dump($date_time);
                        return backpack_url('calendar/' . $date_time->id . '/show');
                    }
                    return backpack_url('user/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                return $entry->start_dt ?? '-';
            });

        CRUD::column('created_at');
        CRUD::column('updated_at');

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
        CRUD::setValidation(TherapistOpenTimeRequest::class);

        CRUD::field('user_id');
        CRUD::field('start_dt');

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
}